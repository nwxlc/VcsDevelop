import React, {useState, useEffect, useCallback} from 'react';

interface AccountData {
    id: string;
    name: string;
    email: string;
    bio: string | null;
    avatarUrl: string | null;
    createdAt: string;
}

const Account: React.FC = () => {
    const [accountData, setAccountData] = useState<AccountData | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    // Берем токен из localStorage, как и на странице репозитория
    const token = localStorage.getItem('accessToken');
    

    // В реальном приложении ID пользователя обычно сохраняется при авторизации или декодируется из JWT-токена
    // Для демонстрации берем сохраненный userId, либо подставляем дефолтный из доки, если локального нет
    const userId = localStorage.getItem('accountId') || '';

    const fetchAccountData = useCallback(async () => {
        setIsLoading(true);
        setError(null);
        try {
            const response = await fetch(`http://localhost:5050/api/account/${userId}`, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token || ''}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const data: AccountData = await response.json();
                setAccountData(data);
            } else {
                setError(`Не удалось загрузить данные профиля. Статус: ${response.status}`);
            }
        } catch (err) {
            console.error('Ошибка при запросе данных аккаунта:', err);
            setError('Произошла сетевая ошибка при загрузке профиля.');
        } finally {
            setIsLoading(false);
        }
    }, [userId, token]); // Функция пересоздастся только при изменении userId или token


// 2. В useEffect передаем зависимость правильно
    useEffect(() => {
        fetchAccountData();
    }, [fetchAccountData]);
    // Красивое форматирование даты создания аккаунта
    const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('ru-RU', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    if (isLoading) {
        return <div className="account-loading">Загрузка данных аккаунта...</div>;
    }

    if (error) {
        return (
            <div className="account-container">
                <div className="account-error-box">
                    <p>{error}</p>
                    <button onClick={fetchAccountData} className="primary-btn small">Повторить попытку</button>
                </div>
            </div>
        );
    }

    return (
        <div className="account-container">
            <div className="account-card">

                {/* Левая колонка: Аватар и базовая инфа */}
                <div className="account-sidebar">
                    <div className="account-avatar-wrapper">
                        {accountData?.avatarUrl ? (
                            <img src={accountData.avatarUrl} alt="Avatar" className="account-avatar" />
                        ) : (
                            // Дефолтная заглушка с первой буквой имени, если аватар null
                            <div className="account-avatar-placeholder">
                                {accountData?.name ? accountData.name.charAt(0).toUpperCase() : 'U'}
                            </div>
                        )}
                    </div>
                    <h2 className="account-name">{accountData?.name}</h2>
                    <span className="account-id-badge">UID: {accountData?.id.substring(0, 8)}...</span>
                </div>

                {/* Правая колонка: Детальные поля из API */}
                <div className="account-info-body">
                    <h3 className="account-section-title">Личный кабинет</h3>
                    <hr className="account-divider" />

                    <div className="account-fields-grid">
                        <div className="account-field-group">
                            <label>Электронная почта</label>
                            <div className="account-field-value">{accountData?.email}</div>
                        </div>

                        <div className="account-field-group">
                            <label>О себе (Bio)</label>
                            <div className="account-field-value bio-field">
                                {accountData?.bio ? accountData.bio : <span className="null-value">Информация отсутствует</span>}
                            </div>
                        </div>

                        <div className="account-field-group">
                            <label>Дата регистрации</label>
                            <div className="account-field-value date-field">
                                {accountData?.createdAt ? formatDate(accountData.createdAt) : ''}
                            </div>
                        </div>
                    </div>

                    <div className="account-actions">
                        <button className="primary-btn small">Редактировать профиль</button>
                    </div>
                </div>

            </div>
        </div>
    );
};

export default Account;