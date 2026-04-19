import React, { useState } from 'react';
import Button from "../../components/GlobalComponents/Button.tsx";
import logo from "../../assets/images/logo.png";
import { Link, useNavigate } from "react-router";

const WorkspaceHeader = () => {
    const [isOpen, setIsOpen] = useState(false);
    const navigate = useNavigate();

    const toggleMenu = () => setIsOpen(!isOpen);

    const logout = () => {
        setIsOpen(false);
        navigate("/");
    };

    return (
        <header className="header">
            <div className="header-title">
                <div className="header-logo">
                    <img src={logo} alt="Logo" />
                </div>
                <Link to="/"><h1>VCS-X</h1></Link>
            </div>

            <div className={`burger-icon ${isOpen ? 'open' : ''}`} onClick={toggleMenu}>
                <span></span>
                <span></span>
                <span></span>
            </div>

            <div className={`side-menu ${isOpen ? 'active' : ''}`}>
                <nav className="menu-links">
                    <Link to="/repositories" onClick={toggleMenu}>Мои репозитории</Link>
                    <Link to="/pullrequests" onClick={toggleMenu}>Pull Requests</Link>
                    <hr />
                    <Button label={"Выйти"} onClick={logout} />
                </nav>
            </div>

            {isOpen && <div className="overlay" onClick={toggleMenu}></div>}
        </header>
    );
};

export default WorkspaceHeader;