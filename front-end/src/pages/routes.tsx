import {Routes, Route} from "react-router-dom";
import { GroupList } from "./GroupPage.tsx";
import LoginPage from "./LoginPage.tsx";

export default function AppRoutes() {
    return(
        <Routes>
            <Route path="/" element={<GroupList />} />
            <Route path="/login" element={<LoginPage />} />
        </Routes>
    )
}