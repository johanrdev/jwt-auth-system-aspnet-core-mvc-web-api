<template>
  <nav class="bg-gray-800 p-4">
    <div class="container mx-auto flex justify-between items-center">
      <router-link to="/" class="text-white text-lg hover:text-gray-300 transition-color duration-300 font-normal">AuthSystem</router-link>
      <div>
        <router-link to="/login" class="text-white hover:text-gray-300 transition-color duration-300 mr-4" v-if="!isLoggedIn">Login</router-link>
        <router-link to="/register" class="text-white hover:text-gray-300 transition-color duration-300 mr-4" v-if="!isLoggedIn">Register</router-link>
        <router-link to="/profile" class="text-white hover:text-gray-300 transition-color duration-300 mr-4" v-if="isLoggedIn">Profile</router-link>
        <button @click="logout" class="text-white hover:text-gray-300 transition-color duration-300 font-semibold" v-if="isLoggedIn">Logout</button>
      </div>
    </div>
  </nav>
  <main class="max-w-5xl w-full my-6 mx-auto flex-grow flex flex-col">
    <router-view @login-success="checkAuth" />
  </main>
  <footer class="bg-gray-900 text-gray-300 p-4">
    <p class="text-center">Copyright &copy;</p>
  </footer>
</template>

<script>
export default {
  data() {
    return {
      isLoggedIn: false
    }
  },
  methods: {
    checkAuth() {
      const token = localStorage.getItem('jwt')
      this.isLoggedIn = !!token
    },
    logout() {
      localStorage.removeItem('jwt')
      this.isLoggedIn = false
      this.$router.push('/login')
    }
  },
  mounted() {
    this.checkAuth()
  }
}
</script>