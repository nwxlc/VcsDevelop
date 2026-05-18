import React, { useState } from 'react';
import Button from "../GlobalComponents/Button.tsx";
import { useNavigate } from "react-router";

const LoginBody: React.FC = () => {
    const [isLoginMode, setIsLoginMode] = useState<boolean>(false);
    const [loading, setLoading] = useState<boolean>(false);
    const [isSubmitted, setIsSubmitted] = useState<boolean>(false);
    const [serverError, setServerError] = useState<string | null>(null);

    // Видимость паролей (раздельная)
    const [showPassword, setShowPassword] = useState<boolean>(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState<boolean>(false);

    const [email, setEmail] = useState<string>("");
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [confirmPassword, setConfirmPassword] = useState<string>("");

    const navigate = useNavigate();

    // Валидация
    const validateEmail = (emailStr: string): boolean => {
        return /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(emailStr);
    };

    const getPasswordStrength = () => {
        if (!password) return null;
        const hasMinLength = password.length >= 8;
        const hasNumbers = /\d/.test(password);
        const hasUpper = /[A-Z]/.test(password);

        if (!hasMinLength) return { label: "слишком короткий", className: "status-weak" };
        if (hasUpper && hasNumbers) return { label: "безопасный", className: "status-safe" };
        return { label: "нормальный", className: "status-normal" };
    };

    const cleanInput = (val: string) => val.replace(/[а-яё]/gi, "");

    const handleSubmit = async () => {
        setIsSubmitted(true);
        setServerError(null);

        if (!validateEmail(email)) return;

        if (!isLoginMode) {
            if (username.trim().length < 2) {
                setServerError("Введите имя пользователя");
                return;
            }
            if (password !== confirmPassword) {
                return; 
            }
        }

        setLoading(true);
        const url = isLoginMode ? "/api/account/login" : "/api/account/registration";
        const payload = isLoginMode ? { email, password } : { email, password, name: username };

        try {
            const response = await fetch(url, {
                method: 'POST',
                body: JSON.stringify(payload),
                headers: { 'Content-Type': 'application/json' },
            });

            if (response.ok) {
                const data = await response.json();
                localStorage.setItem("accessToken", data.accessToken.value);
                localStorage.setItem("accessTokenExpiresAt", data.accessToken.expirationDate);
                localStorage.setItem("refreshToken", data.refreshToken.value);
                localStorage.setItem("accountId", data.accountId);
                navigate("/workspace");
            } else if (response.status === 409) {
                alert("Пользователь с такой почтой уже существует. Переключаемся на вход.");
                setIsLoginMode(true);
            } else {
                const errorData = await response.json().catch(() => ({}));
                setServerError(errorData.message || `Ошибка: ${response.status}`);
            }
        } catch (error) {
            setServerError("Ошибка сети");
        } finally {
            setLoading(false);
        }
    };

    const strength = getPasswordStrength();

    return (
        <div className="login">
            {!isLoginMode && (
                <input
                    value={username}
                    placeholder="имя пользователя"
                    type="text"
                    onChange={(e) => setUsername(cleanInput(e.target.value))}
                />
            )}

            <input
                value={email}
                placeholder="почта"
                type="email"
                onChange={(e) => { setEmail(cleanInput(e.target.value)); setIsSubmitted(false); }}
            />

            {(email.length > 7 || isSubmitted) && !validateEmail(email) && (
                <span className="email-info">
                    <span className="status-text status-weak">почта указана неверно</span>
                </span>
            )}

            {/* Основной пароль */}
            <div className="password-wrapper" style={{ position: 'relative' }}>
                <input
                    placeholder="пароль"
                    type={showPassword ? "text" : "password"}
                    value={password}
                    onChange={(e) => setPassword(cleanInput(e.target.value))}
                />
                <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    style={{ position: 'absolute', right: '10px', top: '33%', transform: 'translateY(-50%)', background: 'none', border: 'none', cursor: 'pointer' }}
                >
                    {showPassword ? "🙈" : "👁️"}
                </button>
            </div>

            {!isLoginMode && (
                <div className="password-wrapper" style={{ position: 'relative' }}>
                    <input
                        placeholder="повторите пароль"
                        type={showConfirmPassword ? "text" : "password"}
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(cleanInput(e.target.value))}
                    />
                    <button
                        type="button"
                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                        style={{ position: 'absolute', right: '10px', top: '33%', transform: 'translateY(-50%)', background: 'none', border: 'none', cursor: 'pointer' }}
                    >
                        {showConfirmPassword ? "🙈" : "👁️"}
                    </button>
                </div>
            )}

            {!isLoginMode && password && strength && (
                <span className="password-info">
                    надёжность: <span className={`status-text ${strength.className}`}>{strength.label}</span>
                </span>
            )}

            {!isLoginMode && isSubmitted && password !== confirmPassword && (
                <span className="status-text status-weak">пароли не совпадают</span>
            )}

            {serverError && <span className="status-text status-weak">Логин или пароль введены неверно</span>}

            <Button
                label={loading ? "загрузка..." : (isLoginMode ? "Войти" : "Зарегистрироваться")}
                onClick={handleSubmit}
            />

            <div className="login-activate" style={{ cursor: 'pointer', marginTop: '15px' }}>
                {isLoginMode ? (
                    <span onClick={() => setIsLoginMode(false)}>Нет аккаунта? <b>Зарегистрироваться</b></span>
                ) : (
                    <span onClick={() => setIsLoginMode(true)}>уже есть аккаунт? <b>войти</b></span>
                )}
            </div>
        </div>
    );
};

export default LoginBody;