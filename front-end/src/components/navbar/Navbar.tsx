import './Navbar.css';
import { useAuth } from '../../context/AuthContext';
import { Link } from 'react-router-dom';

export default function Navbar() {
    const { logout, userId } = useAuth();

    return (
        <div className="navbarContainer">
            <nav className="navbar navbar-dark bg-light">
                <div className="button-container">
                    <ul className="navbar-nav me-auto mb-2 mb-lg-0 d-flex flex-row align-items-center">
                        <li className="nav-item me-3">
                            <Link className="nav-link active text-dark" to="/">Groups</Link>
                        </li>

                        <li className="nav-item me-3">
                            <span className="nav-link active text-dark-emphasis">UserId: {userId}</span>
                        </li>

                        <li className="nav-item ms-auto me-3">
                            <button
                                className="nav-link btn btn-link text-dark"
                                onClick={logout}
                            >
                                Logout
                            </button>
                        </li>
                    </ul>
                </div>
            </nav>
        </div>
    );
}
