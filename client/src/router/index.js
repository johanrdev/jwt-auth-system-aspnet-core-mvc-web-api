import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/HomeView.vue'
import Login from '../views/LoginView.vue'
import Register from '../views/RegisterView.vue'
import Profile from '../views/ProfileView.vue'
import axios from 'axios'

const routes = [
    { path: '/', component: Home },
    { path: '/login', component: Login, meta: { requiresAuth: false } },
    { path: '/register', component: Register, meta: { requiresAuth: false } },
    { path: '/profile', component: Profile, meta: { requiresAuth: true } },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach(async (to, from, next) => {
  try {
    const response = await axios.get('/auth/auth-state', { withCredentials: true })
    const isAuthenticated = response.data.authenticated

    if (to.meta.requiresAuth && !isAuthenticated) {
      next('/login')
    } else if (!to.meta.requiresAuth && isAuthenticated) {
      next('/profile')
    } else {
      next()
    }
  } catch (error) {
    if (to.meta.requiresAuth) {
      next('/login')
    } else {
      next()
    }
  }
})

export default router
