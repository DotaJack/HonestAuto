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

// Initialize SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", function (message) {
    // Handle incoming messages and display them in the chat window
    const chatWindow = document.getElementById("chatWindow");
    chatWindow.innerHTML += `<div><strong>${message.senderId}:</strong> ${message.content}</div>`;
});

connection.start()
    .then(() => console.log("SignalR Connected."))
    .catch(err => console.error(err));