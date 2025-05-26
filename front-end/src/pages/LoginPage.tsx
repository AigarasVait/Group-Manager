import { useState } from "react";
import "./LoginPage.css";
import { validateLogin, createNewLogin } from "../api/membersAPI";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

//  This function is a simple login page component that allows users to enter their credentials
//  and submit them for validation. It uses React hooks for state management 
//  and handles form submission to validate the user's login credentials.
//  It's also not secure in any way, so i would never use it in production.

interface Credentials {
    username: string;
    password: string;
}

export default function LoginPage() {
    const [credentials, setCredentials] = useState<Credentials>({
        username: '',
        password: ''
    });
    const [errorMessage, setErrorMessage] = useState("");
    const navigate = useNavigate();
    const { login } = useAuth();

    const SignIn = async (e: React.FormEvent) => {
        e.preventDefault();
        validateLogin(credentials)
            .then((response) => {
                if (response.id) {
                    login(response.id); 
                    navigate("/");
                } 
            })
            .catch((error) => {
                console.error(error);
                setErrorMessage("Invalid username or password, please try again.");
            });
        return false;
    }

    const Register = async (e: React.FormEvent) => {
        e.preventDefault();
        createNewLogin(credentials)
            .then((response) => {
                if (response.id) {
                    login(response.id); 
                    navigate("/");
                } 
            })
            .catch((error) => {
                console.error(error);
                setErrorMessage("Registration failed: username taken.");
            });
        return false;
    }

    return (
        <div className="login-container">
            <div className="width-cap">
                <form onSubmit={SignIn} className="border border-grey p-3 rounded bg-light">
                    <div className="mb-3">
                        <label htmlFor="username" className="form-label">Username</label>
                        <input
                            onChange={(e) => setCredentials(prev => ({ ...prev, username: e.target.value }))}
                            type="text"
                            className="form-control" id="username" />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="pass" className="form-label">Password</label>
                        <input
                            onChange={(e) => setCredentials(prev => ({ ...prev, password: e.target.value }))}
                            type="password"
                            className="form-control" id="pass" />
                    </div>

                    <div style={{ minHeight: '1.5em' }}>
                        <div className={`form-text text-wrap ${errorMessage ? 'text-danger' : 'invisible'}`}>
                            {errorMessage}
                        </div>
                    </div>

                    <div>
                        <button type="submit" className="btn btn-primary">Sign in</button>
                        <button type="button" onClick={Register} className="btn btn-secondary ms-3">Register</button>
                    </div>

                </form>
            </div>

        </div>
    );
}