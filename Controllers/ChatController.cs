using HonestAuto.Models;
using HonestAuto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HonestAuto.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ChatMessageService _messageService;
        private readonly UserManager<User> _userManager;

        // Source:https://www.youtube.com/watch?v=RUZLIh4Vo20
        // Source 2: https://learn.microsoft.com/en-us/aspnet/signalr/overview/getting-started/tutorial-getting-started-with-signalr
        public ChatController(ChatMessageService messageService, UserManager<User> userManager)
        {
            _messageService = messageService;
            _userManager = userManager;
        }

        // Action to display the chat with a specific user
        public async Task<IActionResult> ChatWithUser(string receiverId)
        {
            // Get the current user's ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve messages between the current user and the receiver
            var messages = await _messageService.GetMessagesForConversationAsync(currentUserId, receiverId);

            // Create a view model to pass messages and receiver ID to the view
            var viewModel = new ChatViewModel
            {
                Messages = messages,
                ReceiverId = receiverId
            };

            // Render the chat view with the view model
            return View(viewModel);
        }

        // Action to handle sending a chat message
        [HttpPost]
        public async Task<IActionResult> SendMessage(ChatViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the sender's ID from the current user's claims
                var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Create a chat message with sender, receiver, content, and timestamp
                var message = new ChatMessage
                {
                    SenderId = senderId,
                    ReceiverId = model.ReceiverId,
                    DateSent = DateTime.UtcNow,
                    Content = model.Content
                };

                // Save the chat message to the database
                await _messageService.SaveMessageAsync(message);

                // You can return a success message or perform other actions here
            }

            // Redirect back to the chat view with the same receiver
            return RedirectToAction("ChatWithUser", new { receiverId = model.ReceiverId });
        }
    }
}