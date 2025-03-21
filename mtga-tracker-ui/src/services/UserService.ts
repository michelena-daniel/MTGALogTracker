import axios from 'axios'

const API_BASE_URL = 'https://localhost:7145/api/UserInfo'

export interface UserInfo {
  mtgaInternalUserId: string
  userNameWithCode: string
  userName: string
  userCode: string
}

export async function getAllUsers(): Promise<UserInfo[]> {
  const response = await axios.get<UserInfo[]>(`${API_BASE_URL}`)
  console.log('User response from service: ', response.data)
  return response.data
}
