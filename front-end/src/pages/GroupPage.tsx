import { useState, useEffect } from "react";
import { useParams, useNavigate } from 'react-router-dom';
import { fetchSingleGroup } from "../api/groupsAPI.ts";
import { useAuth } from "../context/AuthContext.tsx";
import NewTransactionForm from "../components/newTransactionForm/NewTransactionForm.tsx";
import { postTransaction } from "../api/transactionsAPI.ts";
import type { GroupDto } from "../types/Group.ts";
import type { TransactionCreateDto } from "../types/Transaction.ts";
import "./GroupPage.css";

export default function GroupPage() {
    const { groupId } = useParams();
    const { userId, isLoggedIn } = useAuth();
    const [open, setOpen] = useState(false);
    const [groupInfo, setGroupInfo] = useState<GroupDto | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        if (!isLoggedIn || userId === null) {
            navigate("/login", { replace: true });
        }
        else {
            if (!groupId || !userId) return;

            fetchSingleGroup(userId, parseInt(groupId || "0"))
                .then((data) => setGroupInfo(data))
                .catch((error) => {
                    console.error("Failed to fetch group:", error);
                    setGroupInfo(null);
                });
        }

    }, [groupId, userId]);

    if (!groupInfo) {
        return <p className="mx-3 mt-4">Loading group...</p>;
    }

    function handleCreate(newTransaction: TransactionCreateDto) {
        postTransaction(newTransaction)
            .then((newTransactionDb) => {
                setGroupInfo(prevGroupInfo => {
                    if (!prevGroupInfo) return null;

                    return {
                        ...prevGroupInfo,
                        transactions: [...prevGroupInfo.transactions, newTransactionDb]
                    };
                });
            })
            .catch((error) => {
                console.error("Failed to add Transaction:", error);
            });
        setOpen(false);
    }

    return (
        <div className="payment-container">
            <div className="d-flex justify-content-between pb-3 mx-3">
                <p className="h3">{groupInfo.name}</p>
            </div>

            <div className="d-flex justify-content-between pb-1 mx-3">
                <p className="h4">Members:</p>
            </div>
            <div className="table-responsive mx-3">
                <table className="table table-bordered">
                    <thead className="table-light">
                        <tr>
                            <th>Name</th>
                            <th>Balance</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {groupInfo.members?.map((user) => (
                            <tr key={user.id}>
                                <td className="fs-5">{user.name}</td>
                                <td className="fs-5">{user.balance}</td>
                                <td className="fs-5">
                                    <button
                                        className="btn btn-primary"
                                        disabled={user.balance == 0}
                                    >
                                        Pay
                                    </button>
                                    <button
                                        className="btn btn-danger"
                                        disabled={user.balance != 0}
                                    >
                                        Remove
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <div className="d-flex justify-content-between pt-3 pb-1 mx-3">
                <p className="h4">Payments:</p>
                <button onClick={() => setOpen(true)} className="btn btn-primary">
                    Add new Payment
                </button>
            </div>
            <div className="table-responsive mx-3">
                <table className="table table-bordered">
                    <thead className="table-light">
                        <tr>
                            <th>Date</th>
                            <th>Payer</th>
                            <th>Description</th>
                            <th>Amount</th>
                        </tr>
                    </thead>
                    <tbody>
                        {groupInfo.transactions.map((payment) => {
                            const payer = groupInfo.members.find((u) => u.id === payment.payerId);
                            const date = new Date(payment.date);
                            const formatted = date.toISOString().slice(0, 16).replace("T", " ");
                            return (
                                <tr key={payment.id}>
                                    <td className="fs-6">{formatted}</td>
                                    <td className="fs-6">{payer?.name || "Unknown Payer"}</td>
                                    <td className="fs-6 text-break" > {payment.description}</td>
                                    <td className="fs-6">{payment.amount} €</td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>

            {open && (
                <>
                    <div className="dark-overlay" />
                    <NewTransactionForm
                        onCreate={handleCreate}
                        onCancel={() => setOpen(false)}
                        group={groupInfo}
                    />
                </>
            )}
        </div>

    );

}



