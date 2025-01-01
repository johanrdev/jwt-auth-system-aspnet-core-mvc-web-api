<template>
  <div class="flex-grow flex flex-col items-center">
    <h2 class="text-xl font-bold mb-4">Login</h2>
    <form @submit.prevent="login" class="bg-white p-6 rounded-lg shadow-md w-full max-w-sm">
      <div class="mb-4">
        <label for="username" class="block text-gray-700 text-sm font-bold mb-2">Username</label>
        <input v-model="username" id="username" type="text" placeholder="Enter your username" class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline focus:ring-2 focus:ring-indigo-500" />
      </div>
      <div class="mb-4">
        <label for="password" class="block text-gray-700 text-sm font-bold mb-2">Password</label>
        <input v-model="password" id="password" type="password" placeholder="Enter your password" class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline focus:ring-2 focus:ring-indigo-500" />
      </div>
      <div v-if="errorMessage" class="text-red-500 text-sm mb-4">{{ errorMessage }}</div>
      <button class="bg-indigo-500 hover:bg-indigo-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline" type="submit">
        Sign In
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
      password: '',
      errorMessage: ''
    }
  },
  methods: {
    async login() {
      try {
        const response = await axios.post('/auth/login', {
          username: this.username,
          password: this.password
        })
        this.errorMessage = ''
        this.$emit('login-success')
        this.$router.push('/profile')
      } catch (error) {
        this.errorMessage = 'Login failed. Please check your credentials and try again.'
        console.error('Login failed', error)
      }
    }
  }
}
</script>
