<template>
  <div class="navbar">
    <Menubar :model="items" class="mb-4">
      <template #start>
        <img src="/src/assets/SpellTrackerLogo.png" alt="Logo" height="50" />
      </template>
      <template #end>
        <Select
          v-model="userStore.selectedUser"
          :options="userStore.users"
          optionLabel="userName"
          placeholder="Select User"
          class="w-full md:w-56"
        />
      </template>
    </Menubar>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/UserStore'
import Menubar from 'primevue/menubar'
import Select from 'primevue/dropdown'

const userStore = useUserStore()

const router = useRouter()
const items = ref([
  {
    label: 'Home',
    icon: 'pi pi-home',
    command: () => {
      router.push('/')
    },
  },
  {
    label: 'Profile',
    icon: 'pi pi-users',
    command: () => {
      router.push('/profile')
    },
  },
  {
    label: 'Matches',
    icon: 'pi pi-bullseye',
    command: () => {
      router.push('/matches')
    },
  },
  {
    label: 'Settings',
    icon: 'pi pi-cog',
    command: () => {
      router.push('/settings')
    },
  },
])
</script>

<style scoped>
.navbar {
  margin-bottom: 8px;
}

::v-deep(.pi) {
  color: crimson;
}
</style>
