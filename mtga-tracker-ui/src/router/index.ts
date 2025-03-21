import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import MatchView from '@/views/MatchView.vue'
import ProfileView from '@/views/ProfileView.vue'
import SettingsView from '@/views/SettingsView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    { path: '/matches', name: 'Matches', component: MatchView },
    { path: '/profile', name: 'Profile', component: ProfileView },
    { path: '/settings', name: 'Settings', component: SettingsView },
  ],
})

export default router
