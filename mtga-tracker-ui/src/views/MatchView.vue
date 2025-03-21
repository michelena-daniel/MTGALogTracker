<template>
  <div class="matches-layout">
    <!-- Left Column: Matches -->
    <Panel class="scroll-panel">
      <ScrollPanel style="width: 100%; height: 750px">
        <div class="matches-column">
          <h2>Matches</h2>
          <div v-if="matches.length">
            <div v-for="match in matches" :key="match.matchId">
              <Panel
                :class="{
                  'match-card': true,
                  win: match.winnerMtgArenaId === selectedId,
                  lose: match.winnerMtgArenaId !== selectedId,
                }"
                :header="`${match.playerOneName} vs ${match.playerTwoName}`"
              >
                <p><strong>Winner:</strong> {{ match.winnerName }}</p>
              </Panel>
            </div>
          </div>
          <p v-else>No matches found or still loading...</p>
        </div>
      </ScrollPanel>
    </Panel>

    <Panel>
      <div class="chart-column chartLine">
        <Chart type="line" :data="chartData" :options="chartOptions" class="h-[30rem]" />
      </div>
    </Panel>

    <!-- Right Column: Chart -->
    <Panel>
      <div class="chart-column chart">
        <Chart
          type="doughnut"
          :data="chartData"
          :options="chartOptions"
          class="w-full md:w-[30rem]"
        />
      </div>
    </Panel>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getMatchesByArenaId, type Match } from '../services/MatchService'
import { useUserStore } from '../stores/UserStore'
import Panel from 'primevue/panel'
import Chart from 'primevue/chart'
import ScrollPanel from 'primevue/scrollpanel'

const userStore = useUserStore()
const selectedId = userStore.selectedUser?.mtgaInternalUserId

const matches = ref<Match[]>([])

onMounted(async () => {
  try {
    chartData.value = setChartData()
    chartOptions.value = setChartOptions()
    if (selectedId) {
      matches.value = await getMatchesByArenaId(selectedId)
    }
  } catch (error) {
    console.error('Error fetching matches:', error)
  }
})

const chartData = ref()
const chartOptions = ref()

const setChartData = () => {
  const documentStyle = getComputedStyle(document.body)

  return {
    labels: ['Wins', 'Loses'],
    datasets: [
      {
        data: [31, 22],
        backgroundColor: [
          documentStyle.getPropertyValue('--p-green-500'),
          documentStyle.getPropertyValue('--p-red-500'),
          documentStyle.getPropertyValue('--p-gray-500'),
        ],
        hoverBackgroundColor: [
          documentStyle.getPropertyValue('--p-green-400'),
          documentStyle.getPropertyValue('--p-red-400'),
          documentStyle.getPropertyValue('--p-gray-400'),
        ],
      },
    ],
  }
}

const setChartOptions = () => {
  const documentStyle = getComputedStyle(document.documentElement)
  const textColor = documentStyle.getPropertyValue('--p-text-color')

  return {
    plugins: {
      legend: {
        labels: {
          cutout: '60%',
          color: textColor,
        },
      },
    },
  }
}
</script>

<style scoped>
.matches-layout {
  display: flex;
  gap: 20px;
  align-items: flex-start;
}

.matches-column {
  flex: 1;
}

.chart-column {
  flex: 1;
  max-width: 600px;
}

.match-card {
  margin: 5px;
  max-width: 400px;
}

.match-card.win {
  background-color: #d4edda;
  color: #155724;
}

.match-card.win:hover {
  background-color: #89e29e;
}

.match-card.lose {
  background-color: #f8d7da;
  color: #721c24;
}

.match-card.lose:hover {
  background-color: #f59da4;
}

.panel-content {
  display: flex;
  align-items: center;
  gap: 12px;
}

.panel-image {
  width: 48px;
  height: 48px;
  object-fit: contain;
}

.chart {
  max-width: 400px;
  max-height: 500px;
}

.chartLine {
  max-width: 500px;
  max-height: 1000px;
}

.scroll-panel {
  max-width: 500px;
}

h2 {
  text-align: center;
}
</style>
