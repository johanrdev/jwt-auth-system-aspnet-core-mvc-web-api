<template>
  <div class="container mx-auto">
    <h1 class="text-2xl">Profile</h1>
    <p>Username: {{ username }}</p>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  data() {
    return {
      username: ''
    }
  },
  async mounted() {
    try {
      const response = await axios.get('/auth/auth-state', { withCredentials: true })
      this.username = response.data.username
    } catch (error) {
      console.error('Auth-state check failed', error.response.data)
      this.$router.push('/login')
    }
  }
}
</script>
