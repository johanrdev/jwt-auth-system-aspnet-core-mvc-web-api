<template>
    <div class="flex-grow flex flex-col items-center">
      <h2 class="text-xl font-bold mb-4">Register</h2>
      <form @submit.prevent="register" class="bg-white p-6 rounded-lg shadow-md w-full max-w-sm">
        <div class="mb-4">
          <label for="username" class="block text-gray-700 text-sm font-bold mb-2">Username</label>
          <input v-model="username" id="username" type="text" placeholder="Enter your username" class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline focus:ring-2 focus:ring-indigo-500" />
          <div v-if="usernameError" class="text-red-500 text-sm">{{ usernameError }}</div>
        </div>
        <div class="mb-4">
          <label for="email" class="block text-gray-700 text-sm font-bold mb-2">Email</label>
          <input v-model="email" id="email" type="email" placeholder="Enter your email" class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline focus:ring-2 focus:ring-indigo-500" />
          <div v-if="emailError" class="text-red-500 text-sm">{{ emailError }}</div>
        </div>
        <div class="mb-4">
          <label for="password" class="block text-gray-700 text-sm font-bold mb-2">Password</label>
          <input v-model="password" id="password" type="password" placeholder="Enter your password" class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline focus:ring-2 focus:ring-indigo-500" />
          <div v-if="passwordError" class="text-red-500 text-sm">{{ passwordError }}</div>
        </div>
        <div v-if="errorMessages.length" class="text-red-500 text-sm mb-4">
          <ul>
            <li v-for="(message, index) in errorMessages" :key="index">{{ message }}</li>
          </ul>
        </div>
        <button class="bg-indigo-500 hover:bg-indigo-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline" type="submit">
          Register
        </button>
      </form>
    </div>
  </template>
  
  <script>
  import axios from 'axios'
  
  export default {
    data() {
      return {
        username: '',
        email: '',
        password: '',
        errorMessages: [],
        usernameError: '',
        emailError: '',
        passwordError: ''
      }
    },
    methods: {
      async register() {
        this.clearErrors()
  
        if (!this.validateFields()) {
          return
        }
  
        try {
          const response = await axios.post('/auth/register', {
            username: this.username,
            email: this.email,
            password: this.password
          })
          if (response.status === 200) {
            this.errorMessages = [] // Clear any previous error messages
            this.$router.push('/login')
          }
        } catch (error) {
          if (error.response && error.response.data) {
            this.errorMessages = error.response.data.map(err => err.description)
          } else {
            this.errorMessages = ['Registration failed. Please try again.']
          }
          console.error('Registration failed', error)
        }
      },
      clearErrors() {
        this.errorMessages = []
        this.usernameError = ''
        this.emailError = ''
        this.passwordError = ''
      },
      validateFields() {
        let isValid = true
  
        // Username validation
        const usernamePattern = /^[a-zA-Z][a-zA-Z0-9]{2,19}$/
        if (!this.username) {
          this.usernameError = 'Username is required.'
          isValid = false
        } else if (!usernamePattern.test(this.username)) {
          this.usernameError = 'Username must be alphanumeric, between 3 and 20 characters, and cannot start with a number.'
          isValid = false
        }
  
        // Email validation
        const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
        if (!this.email) {
          this.emailError = 'Email is required.'
          isValid = false
        } else if (!emailPattern.test(this.email)) {
          this.emailError = 'Invalid email address.'
          isValid = false
        }
  
        // Password validation
        if (!this.password) {
          this.passwordError = 'Password is required.'
          isValid = false
        }
  
        return isValid
      }
    }
  }
  </script>
  