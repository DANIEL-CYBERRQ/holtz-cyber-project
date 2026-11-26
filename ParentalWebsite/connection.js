const socket = new WebSocket("ws://localhost:5000");

socket.addEventListener("open", () => {
    console.log("connected to server");
});

socket.addEventListener("error", () => {
    console.log("connection error - is the server running?");
});