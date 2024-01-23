import style from './NetworkBtn.module.css'

interface NetworkBtnProps {
    onBtnClicked: () => void
    isBtnPending: boolean
}

const  NetworkBtn = ({onBtnClicked, isBtnPending}: NetworkBtnProps): JSX.Element => {
    const onNetworkBtnClicked = (evt: React.MouseEvent<HTMLElement>): void => {
        evt.stopPropagation();
        evt.preventDefault();

        if(isBtnPending)
        {
            return;
        }
        
        onBtnClicked();
    };


    return (
        <div className={style['network-btn']} onClick={onNetworkBtnClicked}>
            <div 
                className={`icon ${isBtnPending ? style['waiting-icon']: style['network-icon']} ${style['btn-background']} ${isBtnPending ? '' : style['idling']}`} 
                style={{ cursor: isBtnPending ? 'wait': 'pointer' }}
                title="Click To Check Proxy">
            </div>
        </div>
    );
}

export default NetworkBtn;