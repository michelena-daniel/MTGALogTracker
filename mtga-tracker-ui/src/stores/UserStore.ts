import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { UserInfo } from '../services/UserService'
import { getAllUsers } from '../services/UserService'

export const useUserStore = defineStore('user', () => {
  const users = ref<UserInfo[]>([])
  const selectedUser = ref<UserInfo | null>(null)

  async function fetchUsers() {
    users.value = await getAllUsers()
    console.log('Fetched users:', users.value)
    selectedUser.value = users.value[0] || null
    console.log('Selected user:', selectedUser.value)
  }

  function setSelectedUser(user: UserInfo) {
    selectedUser.value = user
  }

  return {
    users,
    selectedUser,
    fetchUsers,
    setSelectedUser,
  }
})
