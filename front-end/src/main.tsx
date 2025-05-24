import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import 'bootstrap/dist/css/bootstrap.min.css';
import { GroupList } from './pages/GroupPage.tsx'
import Navbar from './components/navbar/Navbar.tsx'
import './index.css'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Navbar />
    <GroupList />
  </StrictMode>,
)
