<template>
  <Timeline :value="events">
    <template #content="slotProps">
      {{ slotProps.item.status }}
    </template>
  </Timeline>
</template>

<script setup lang="ts">
import Timeline from 'primevue/timeline'
import { ref } from 'vue'

const events = ref([
  { status: 'Play', date: '15/10/2020 10:30', color: '#9C27B0' },
  { status: 'Track', date: '15/10/2020 14:00', color: '#673AB7' },
  { status: 'Monitor', date: '15/10/2020 16:15', color: '#FF9800' },
  { status: 'Improve', date: '16/10/2020 10:00', color: '#607D8B' },
])
</script>
