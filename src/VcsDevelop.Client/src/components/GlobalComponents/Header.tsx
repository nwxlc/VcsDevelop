import React, { useEffect, useState } from 'react';
import Button from "../../components/GlobalComponents/Button.tsx";
import logo from "../../assets/images/logo.png";
import { Link, useLocation } from "react-router";
import {useAuth} from "../../hooks/useAuth.ts";

const Header = () => {
    const location = useLocation();
    const { refreshToken } = useAuth();
    const [isAuth, setIsAuth] = useState(!!localStorage.getItem("accessToken"));

    useEffect(() => {
        const checkToken = async () => {
            const token = localStorage.getItem("accessToken");
            if (!token && localStorage.getItem("refreshToken")) {
                const newToken = await refreshToken();
                setIsAuth(!!newToken);
            } else {
                setIsAuth(!!token);
            }
        };

        checkToken();
    }, [location.pathname]); // Проверяем при переходах

    return (
        <div className="header">
            <div className="header-title">
                <div className="header-logo">
                    <img src={logo} alt="Logo" />
                </div>
                <Link to="/"><h1>VCS-X</h1></Link>
            </div>

            {location.pathname === "/login" ? null : (
                isAuth ? (
                    <Link to="/workspace">
                        <Button label={"Личный кабинет"} onClick={() => {}} />
                    </Link>
                ) : (
                    <Link to="/login">
                        <Button label={"Авторизация"} onClick={() => {}} />
                    </Link>
                )
            )}
        </div>
    );
};

export default Header;