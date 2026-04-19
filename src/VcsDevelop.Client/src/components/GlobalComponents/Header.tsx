import React from 'react';
import Button from "../../components/GlobalComponents/Button.tsx";
import logo from "../../assets/images/logo.png";
import { Link, useLocation } from "react-router";

const Header = () => {
    const location = useLocation();

    return (
        <div className="header">
            <div className="header-title">
                <div className="header-logo">
                    <img src={logo} alt="Logo" />
                </div>
                <Link to="/"><h1>VCS-X</h1></Link>
            </div>
            {location.pathname === "/login" ? (<></>) : (
                <Link to="/login">
                    <Button label={"Авторизация"} onClick={() => {}} />
                </Link>
            )}

        </div>
    );
};

export default Header;