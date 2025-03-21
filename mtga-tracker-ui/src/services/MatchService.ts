import axios from 'axios'
import type { UserInfo } from './UserService'

const API_BASE_URL = 'https://localhost:7145/api/Match'

export interface Match {
  matchId: string
  requestId?: number
  transactionId?: string
  timeStamp: string
  matchCompletedReason: string
  isDraw: boolean
  winningTeamId: number
  playerOneName: string
  playerTwoName: string
  playerOneMtgaId: string
  playerTwoMtgaId: string
  homeUser?: string
  winnerMtgArenaId?: string
  winnerName?: string
  user: UserInfo
}

export async function getMatchesByArenaId(mtgArenaId: string): Promise<Match[]> {
  const response = await axios.get<Match[]>(`${API_BASE_URL}/${mtgArenaId}`)
  console.log('Match response from service: ', response.data)
  return response.data
}
