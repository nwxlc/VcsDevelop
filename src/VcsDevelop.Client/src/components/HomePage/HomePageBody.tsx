import React, {Component} from 'react';
import background from "../../assets/images/image-2.png"
import info1 from "../../assets/images/image-1.png"
import info2 from "../../assets/images/image-3.png"
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
                <p className="home-block">
                    Лучшее рабочее пространство для контроля версий <br/> и совместной разработки
                </p>
                <div className="home-backgroundImage bounce-box">
                    <img src={background} alt=""/>
                </div>
                
                <div className="home-block home-info home-info-first">
                    <div className="home-info-img">
                        <img src={info1} alt=""/>
                    </div>
                    <div className="home-info-text">
                        <p>
                            работайте из любой точки планеты. быстрый и стабильный доступ к вашим проектам и инструменты для контроля версий.
                        </p>
                    </div>
                </div>

                <div className="home-block home-info home-info-second">
                    <div className="home-info-text">
                        <p>
                            делегируйте рабочий процесс своим коллегам и делитесь проектами за считанные секунды без лишних действий.
                        </p>
                    </div>
                    <div className="home-info-img">
                        <img src={info2} alt=""/>
                    </div>
                </div>
            </div>
        );
    }
}

export default HomePageBody;