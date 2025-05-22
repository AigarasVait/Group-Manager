import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { MembersList } from './groupPage.ts'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MembersList />
  </StrictMode>,
)
