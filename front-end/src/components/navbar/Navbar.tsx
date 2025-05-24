import React from 'react';
import './Navbar.css';

export default function Navbar() {
    return (
        <div className="navbarContainer">
            <nav className="navbar navbar-dark bg-light">
                <div className="button-container">
                    <ul className="navbar-nav me-auto mb-2 mb-lg-0 d-flex flex-row">
                        <li className="nav-item me-3">
                            <a className="nav-link active text-dark" href="/">Groups</a>
                        </li>
                        <li className="nav-item me-3">
                            <a className="nav-link active text-dark" href="/other">Another tab</a>
                        </li>
                    </ul>
                </div>
            </nav>
        </div>
    );

}