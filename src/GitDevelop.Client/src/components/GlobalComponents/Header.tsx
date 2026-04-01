import React, {Component} from 'react';
import Button from "../../components/GlobalComponents/Button.tsx";
import logo from "../../assets/images/logo.png";

class Header extends Component {
    render() {
        return (
            <div className="header">
                <div className="header-title">
                    <div className="header-logo"><img src={logo} alt=""/></div>
                    <h1>VCS-X</h1>
                </div>
                
                <Button label={"Авторизация"} onClick={''}/>  
            </div>
        );
    }
}

export default Header;