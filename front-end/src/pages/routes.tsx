import {Routes, Route} from "react-router-dom";
import GroupListPage from "./GroupListPage.tsx";
import GroupPage from "./GroupPage.tsx";
import LoginPage from "./LoginPage.tsx";

export default function AppRoutes() {
    return(
        <Routes>
            <Route path="/" element={<GroupListPage />} />
            <Route path="/groups" element={<GroupListPage />} />
            <Route path="/group/:groupId" element={<GroupPage />} />
            <Route path="/login" element={<LoginPage />} />
        </Routes>
    )
}