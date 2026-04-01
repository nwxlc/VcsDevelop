import React, {Component} from 'react';
import background from "../../assets/images/image-2.png"
class HomePageBody extends Component {
    render() {
        return (
            <div className="home">
                <div className="home-greetings">
                    Просим любить и жаловать <br/>
                    <h1 className="home-title">
                        VSC-X
                    </h1>
                </div>
                <div className="home-description">
                    Лучшее рабочее пространство для контроля версий <br/> и совместной разработки
                </div>
                <div className="home-backgroundImage bounce-box">
                    <img src={background} alt=""/>
                </div>
            </div>
        );
    }
}

export default HomePageBody;