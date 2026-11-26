const form = document.getElementById('form')
const fullname_input = document.getElementById('fullname-input')
const email_input = document.getElementById('email-input')
const password_input = document.getElementById('password-input')
const repeat_password = document.getElementById('repeat-password')
const error_message = document.getElementById('error-message')

form.addEventListener('submit', (e) => {
   e.preventDefault()   // תמיד מונע רענון של הדף

   // 1. בדיקת שדות
   let errors = []
   if (fullname_input) {
     errors = getSignupFormErrors(fullname_input.value, email_input.value, password_input.value, repeat_password.value)
   } else {
     errors = getLoginFormErrors(email_input.value, password_input.value)
   }
   if (errors.length > 0) {
     error_message.style.color = ""
     error_message.innerText = errors.join(". ")
     return
   }

   // 2. ודא שיש חיבור לשרת
   if (socket.readyState !== WebSocket.OPEN) {
     error_message.innerText = "אין חיבור לשרת. ודא שהשרת רץ."
     return
   }

   // 3. שלח לשרת
   if (fullname_input) {
     socket.send(JSON.stringify({
       type: "register",
       data: { name: fullname_input.value, email: email_input.value, password: password_input.value }
     }))
   } else {
     socket.send(JSON.stringify({
       type: "login",
       data: { email: email_input.value, password: password_input.value }
     }))
   }
})

// טיפול בתשובה מהשרת
socket.addEventListener("message", (e) => {
   const response = JSON.parse(e.data)

   if (response.type === "ok") {
     error_message.style.color = "green"
     if (fullname_input) {
       error_message.innerText = "נרשמת בהצלחה! מעביר לדף התחברות..."
       setTimeout(() => { window.location.href = "login.html" }, 1500)
     } else {
       error_message.innerText = "התחברת בהצלחה!"
       // בהמשך נעביר לדשבורד: window.location.href = "dashboard.html"
     }
   } else {
     error_message.style.color = ""
     error_message.innerText = response.data.message
   }
})

function getSignupFormErrors(fullname, email, password, repeatpassword) {
   let errors = []
   if (fullname === '' || fullname == null) { errors.push('Fullname is required'); fullname_input.parentElement.classList.add('incorrect') }
   if (email === '' || email == null) { errors.push('Email is required'); email_input.parentElement.classList.add('incorrect') }
   if (password === '' || password == null) { errors.push('Password is required'); password_input.parentElement.classList.add('incorrect') }
   if (password !== repeatpassword) { errors.push('Passwords do not match'); password_input.parentElement.classList.add('incorrect'); repeat_password.parentElement.classList.add('incorrect') }
   return errors
}

function getLoginFormErrors(email, password) {
   let errors = []
   if (email === '' || email == null) { errors.push('Email is required'); email_input.parentElement.classList.add('incorrect') }
   if (password === '' || password == null) { errors.push('Password is required'); password_input.parentElement.classList.add('incorrect') }
   return errors
}

const allInputs = [fullname_input, email_input, password_input, repeat_password].filter(input => input != null)
allInputs.forEach(input => {
   input.addEventListener('input', () => {
     if (input.parentElement.classList.contains('incorrect')) {
       input.parentElement.classList.remove('incorrect')
       error_message.innerText = ''
     }
   })
})