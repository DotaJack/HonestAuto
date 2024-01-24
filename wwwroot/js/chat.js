// Source:https://www.youtube.com/watch?v=RUZLIh4Vo20
// Source 2: https://learn.microsoft.com/en-us/aspnet/signalr/overview/getting-started/tutorial-getting-started-with-signalr

function sendMessage(receiverId, messageContent) {
    connection.invoke("SendMessage", receiverId, messageContent)
        .then(() => {
            // Clear the input field after sending
            document.getElementById("messageInput").value = '';
        })
        .catch(err => console.error(err.toString()));
}

// Event listener for form submission
document.getElementById("messageForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent the form from submitting the traditional way

    const receiverId = document.getElementById("receiverId").value;
    const messageContent = document.getElementById("messageInput").value;

    if (!messageContent.trim()) return; // Don't send an empty message

    sendMessage(receiverId, messageContent);
});
// Function to append a sent message to the chat window
function appendSentMessage(messageContent) {
    const chatWindow = document.getElementById("chatWindow");
    const messageDiv = document.createElement("div");
    messageDiv.innerHTML = `<strong>You:</strong> ${messageContent}`;
    chatWindow.appendChild(messageDiv);

    // Scroll to the bottom of the chat window to show the latest message
    chatWindow.scrollTop = chatWindow.scrollHeight;
}
// Initialize SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", function (message) {
    const chatWindow = document.getElementById("chatWindow");
    const messageDiv = document.createElement("div");

    if (message.sender !== null && message.sender.userName === userIdentifier) {
        messageDiv.innerHTML = `<strong>You:</strong> ${message.content} <br /><small>${message.dateSent}</small>`;
    } else {
        messageDiv.innerHTML = `<strong>${message.sender?.userName}:</strong> ${message.content} <br /><small>${message.dateSent}</small>`;
    }

    chatWindow.appendChild(messageDiv);

    // Scroll to the bottom of the chat window to show the latest message
    chatWindow.scrollTop = chatWindow.scrollHeight;
});

connection.start()
    .then(() => console.log("SignalR Connected."))
    .catch(err => console.error(err));