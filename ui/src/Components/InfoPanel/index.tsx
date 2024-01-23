import style from './InfoPanel.module.css'

export enum ProxyStatus {
    UnChecked,
    Online,
    Down,
}

interface InfoPanelProps {
    proxyStatus: ProxyStatus
    proxyUrl: string
}

const InfoPanel = ({proxyStatus, proxyUrl}: InfoPanelProps):JSX.Element => {
    const status = {
        color: '',
        text: '',
    };

    switch(proxyStatus)
    {
        case ProxyStatus.UnChecked: {
            status.color = `#24403e`;
            status.text = `Not Checked`;
            break;
        }
        case ProxyStatus.Online: {
            status.color = `rgb(81, 193, 81)`;
            status.text = `Alive`;
            break;
        }
        case ProxyStatus.Down: {
            status.color = `rgb(82, 21, 21)`;
            status.text = `Not Connected`;
            break;
        }
    }

    return (
        <div className={style['info-panel']}>
            <div className={style['title']}>
                ProxyChker Utility
            </div>
            
            <div className={style['main-content']}>
                <div className={style['item']}> 
                    <div className={style['title']}>
                        Current Proxy
                    </div>
                    <div className={style['value']}>
                        { proxyUrl }
                    </div>
                </div>

                <div className={style['item']}> 
                    <div className={style['title']}>
                        Proxy Status
                    </div>
                    <div className={`${style['value']}`} style={{ color: status.color }}>
                        { status.text }
                    </div>
                </div>

            </div>
        </div>
    )
};

export default InfoPanel;