import React, {useState} from 'react';
import Button from "../GlobalComponents/Button.tsx";

const LoginBody = () => {
    const [password, setPassword] = useState("");
    const [email, setEmail] = useState("");

    const validateEmail = (email) => {
        const regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return regex.test(email);
    };

    const handleEmail = (e) => {
        const val = e.target.value;
        const cleanValue = val.replace(/[а-яё]/gi, "");
        setEmail(cleanValue);
    };

    const handlePassword = (e) => {
        const val = e.target.value;
        const cleanValue = val.replace(/[а-яё]/gi, "");
        setPassword(cleanValue);
    };

    const getEmailData = (emailVal) => {
        if (!emailVal) return null;
        if (!validateEmail(emailVal)) {
            return {label: "почта указана неверно", className: "status-weak"};

        }
    };

    const getStrengthData = (pwd) => {
        if (!pwd) return null;
        const hasMinLength = pwd.length >= 8;
        const hasLatin = /[a-zA-Z]/.test(pwd);
        const hasNumbers = /\d/.test(pwd);
        const hasUpper = /[A-Z]/.test(pwd);

        if (!hasMinLength || !hasLatin) {
            return {label: "слишком короткий", className: "status-weak"};
        }
        if (hasUpper && hasNumbers) {
            return {label: "безопасный", className: "status-safe"};
        }
        if (hasNumbers) {
            return {label: "нормальный", className: "status-normal"};
        }
        return {label: "слабый", className: "status-weak"};
    };

    const emailStatus = getEmailData(email);
    const passwordStrength = getStrengthData(password);

    return (
        <div className="login">
            <input
                id="email"
                value={email}
                placeholder="почта"
                type="email"
                onChange={handleEmail}
            />
            {email.length > 7 && emailStatus && (
                <span className="email-info">
                    <span className={`status-text ${emailStatus.className}`}>
                        {emailStatus.label}
                    </span>
                </span>
            )}

            <input
                id="password"
                placeholder="пароль"
                type="password"
                value={password}
                onChange={handlePassword}
            />
            {password && passwordStrength && (
                <span className="password-info">
                    надёжность пароля:{" "}
                    <span className={`status-text ${passwordStrength.className}`}>
                        {passwordStrength.label}
                    </span>
                </span>
            )}

            <Button label="войти" onClick={() => console.log("TEST")}/>
        </div>
    );
};

export default LoginBody;