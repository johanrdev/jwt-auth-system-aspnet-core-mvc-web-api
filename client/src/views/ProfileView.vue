<template>
    <div class="container mx-auto">
      <h1 class="text-2xl">Profile</h1>
      <p>Username: {{ username }}</p>
    </div>
  </template>
  
  <script>
import { jwtDecode } from "jwt-decode"
  
  export default {
    data() {
      return {
        username: ''
      }
    },
    mounted() {
      const token = localStorage.getItem('jwt')
      if (token) {
        const decodedToken = jwtDecode(token)
        this.username = decodedToken.sub
      } else {
        this.$router.push('/login')
      }
    }
  }
  </script>
  