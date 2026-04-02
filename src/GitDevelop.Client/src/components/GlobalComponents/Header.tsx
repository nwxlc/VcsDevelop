import React, {Component} from 'react';
import Button from "../../components/GlobalComponents/Button.tsx";
import logo from "../../assets/images/logo.png";

import {Link} from "react-router";


class Header extends Component {
    render() {
        return (
            <div className="header">
                <div className="header-title">
                    <div className="header-logo"><img src={logo} alt=""/></div>
                    <Link to="/"><h1>VCS-X</h1></Link>
                </div>
                <Link to="/login">
                    <Button label={"Авторизация"} onClick={()=>{}}/>  
                </Link>
                
            </div>
        );
    }
}

export default Header;