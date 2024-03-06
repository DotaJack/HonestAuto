﻿using HonestAuto.Models;
using HonestAuto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace HonestAuto.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ChatMessageService _messageService;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;

        // Source:https://www.youtube.com/watch?v=RUZLIh4Vo20
        // Source 2: https://learn.microsoft.com/en-us/aspnet/signalr/overview/getting-started/tutorial-getting-started-with-signalr
        public ChatController(ChatMessageService messageService, UserManager<User> userManager, EmailService emailService)

        {
            _messageService = messageService;
            _userManager = userManager;
            _emailService = emailService;
        }

        // Action to display the list of conversations for the current user
        public async Task<IActionResult> Index()
        {
            // Get the current user's ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve conversations for the current user
            var conversations = await _messageService.GetConversationsAsync(currentUserId);

            // Fetch sender usernames for each conversation and group by receiver ID
            var conversationViewModels = new List<ChatViewModel>();
            foreach (var conversation in conversations)
            {
                var receiverId = conversation.ReceiverId == currentUserId ? conversation.SenderId : conversation.ReceiverId;
                var receiverUsername = await _messageService.FetchUsernameForIdAsync(receiverId);

                conversationViewModels.Add(new ChatViewModel
                {
                    ReceiverId = receiverId,
                    ReceiverUsername = receiverUsername,
                    Content = conversation.Content,
                    // Add the sender's ID here if needed
                    SenderId = conversation.SenderId
                });
            }

            // Pass the list of conversations to the view
            return View(conversationViewModels);
        }

        public IActionResult StartChat(string id)
        {
            // Perform any necessary logic to set up the chat, e.g., create a conversation
            // and navigate to the chat view.
            // You can also pass the user's ID to the chat view if needed.
            return RedirectToAction("ChatWithUser", new { receiverId = id });
        }

        // Action to display the chat with a specific user
        public async Task<IActionResult> ChatWithUser(string receiverId)
        {
            // Get the current user's ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the receiver's username based on the receiverId
            var receiverUsername = await _messageService.FetchUsernameForIdAsync(receiverId);

            // Retrieve messages between the current user and the receiver
            var messages = await _messageService.GetMessagesForConversationAsync(currentUserId, receiverId);

            // Create a view model to pass messages, receiver ID, and receiver username to the view
            var viewModel = new ChatViewModel
            {
                Messages = messages,
                ReceiverId = receiverId,
                ReceiverUsername = receiverUsername // Set the ReceiverUsername property
            };

            // Render the chat view with the view model
            return View(viewModel);
        }

        // Action to handle sending a chat message
        [HttpPost]
        [ValidateAntiForgeryToken] // Ensure that the request is coming from your own form
        public async Task<IActionResult> SendMessage(ChatViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the sender's ID from the current user's claims
                    var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Create a chat message with sender, receiver, content, and timestamp
                    var message = new ChatMessage
                    {
                        SenderId = senderId,
                        ReceiverId = model.ReceiverId, // Use model.ReceiverId to set the receiver ID
                        DateSent = DateTime.UtcNow,
                        Content = model.Content
                    };

                    // Save the chat message to the database
                    await _messageService.SaveMessageAsync(message);

                    // Fetch the receiver's email address using UserManager
                    var receiver = await _userManager.FindByIdAsync(model.ReceiverId); // Find the receiver by ID
                    var receiverEmail = receiver != null ? await _userManager.GetEmailAsync(receiver) : null; // Get the receiver's email

                    var emailSubject = "New Chat Message";
                    var emailContent = $"You have received a new message from .";
                    await _emailService.SendEmailAsync(receiverEmail, emailSubject, emailContent);

                    // You can return a success message or perform other actions here
                }
                catch (Exception ex)
                {
                    // Log the exception for troubleshooting
                    Console.WriteLine($"An error occurred while sending the message: {ex.Message}");
                    // You may choose to handle the error differently, e.g., show an error message to the user
                }
            }

            // Redirect back to the chat view with the same receiver
            return RedirectToAction("ChatWithUser", new { receiverId = model.ReceiverId });
        }
    }
}