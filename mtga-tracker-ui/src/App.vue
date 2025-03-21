<template>
  <div>
    <NavBar />
    <router-view />
  </div>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router'
import NavBar from './components/NavBar.vue'
import { useUserStore } from './stores/UserStore'
import { onMounted } from 'vue'

const userStore = useUserStore()

onMounted(() => {
  userStore.fetchUsers()
})
</script>
