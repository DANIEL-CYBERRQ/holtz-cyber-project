(function () {
    const parentId = sessionStorage.getItem("parentId")
const parentName = sessionStorage.getItem(parentName)

if (!parentId) {
    window.location.href = "login.html"
    return
}

const greeting = document.getElementById("greeting")
if (greeting){
    greeting.innerText = "Welcome " + (parentName || "")
}

const logoutBtn = document.getElementById(logout-btn)
logoutBtn.addEventListener("click", () => {
    sessionStorage.clear()
    window.location.href = "login.html"
})

})

