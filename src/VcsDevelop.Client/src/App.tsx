import './App.css'
import { useEffect } from "react";
import { Routes, Route, useNavigate, useLocation } from "react-router";
import HomePage from "./pages/HomePage.tsx";
import Login from "./pages/Login.tsx";
import Workspace from "./pages/Workspace.tsx";
import {useAuth} from "./hooks/useAuth.ts";
import Repositories from "./pages/Repositories.tsx";
import PullRequests from "./pages/PullRequests.tsx";
import Repository from "./pages/Repository.tsx";


function App() {
    const navigate = useNavigate();
    const location = useLocation();
    const { refreshToken } = useAuth();
    const accessToken = localStorage.getItem("accessToken");
    console.log(accessToken)

    useEffect(() => {
        const protectedRoutes = ["/workspace"];
        const isProtectedRoute = protectedRoutes.includes(location.pathname);

        const verify = async () => {
            const token = localStorage.getItem("accessToken");
            if (isProtectedRoute && !token) {
                const success = await refreshToken();
                if (!success) {
                    alert("Нужна авторизация");
                    navigate("/login");
                }
            }
        };

        verify();
    }, [location.pathname, navigate]);

    return (
        <Routes>
            <Route path="/" element={<HomePage/>}/>
            <Route path="/login" element={<Login/>}/>
            <Route path="/workspace" element={<Workspace/>}/>
            <Route path="/repositories" element={<Repositories/>}/>
            <Route path="/repository/:name" element={<Repository/>} />
            <Route path="/pullrequests" element={<PullRequests/>}/>
        </Routes>
    )
}

export default App