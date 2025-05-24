import React from 'react';
import './Navbar.css';
import { useAuth } from '../../context/AuthContext';

export default function Navbar() {
    const { logout } = useAuth();
    return (
        <div className="navbarContainer">
            <nav className="navbar navbar-dark bg-light">
                <div className="button-container">
                    <ul className="navbar-nav me-auto mb-2 mb-lg-0 d-flex flex-row ">
                        <li className="nav-item me-3">
                            <a className="nav-link active text-dark" href="/">Groups</a>
                        </li>
                        
                        <li className="nav-item ms-auto me-3">
                            <a className="nav-link active text-dark" onClick={logout}>Logout</a>
                        </li>
                    </ul>

                </div>
            </nav>
        </div>
    );

}