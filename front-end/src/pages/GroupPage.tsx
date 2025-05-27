import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { fetchSingleGroup, patchGroupMember } from "../api/groupsAPI.ts";
import { postTransaction } from "../api/transactionsAPI.ts";
import { useAuth } from "../context/AuthContext.tsx";

import NewTransactionForm from "../components/newTransactionForm/NewTransactionForm.tsx";
import type { GroupDto } from "../types/Group.ts";
import type { TransactionCreateDto } from "../types/Transaction.ts";

import "./GroupPage.css";

export default function GroupPage() {
    const { groupId } = useParams();
    const navigate = useNavigate();
    const { userId, isLoggedIn } = useAuth();
    const [formUser, setFormUser] = useState<number>(0);

    const [groupInfo, setGroupInfo] = useState<GroupDto | null>(null);
    const [open, setOpen] = useState(false);
    const [newMemberId, setNewMemberId] = useState("");

    useEffect(() => {
        if (!isLoggedIn || userId == null) {
            navigate("/login", { replace: true });
            return;
        }

        if (!groupId) return;

        const fetchGroup = async () => {
            try {
                const groupData = await fetchSingleGroup(userId, parseInt(groupId));
                setGroupInfo(groupData);
            } catch (err) {
                console.error("Failed to fetch group:", err);
                setGroupInfo(null);
            }
        };

        fetchGroup();
    }, [groupId, userId]);

    if (!groupInfo) {
        return <p className="mx-3 mt-4">Loading group...</p>;
    }

    const handleCreateTransaction = async (newTransaction: TransactionCreateDto) => {
        try {
            const savedTransaction = await postTransaction(newTransaction);

            // Dynamically updated local transaction list, but still reloaded to get updated balances.
            setGroupInfo(prev => (
                prev ? {
                    ...prev,
                    transactions: [...prev.transactions, savedTransaction]
                } : null
            ));
        } catch (err) {
            console.error("Failed to add Transaction:", err);
        }

        window.location.reload(); // Needed for updated balances, should have made the API return members or something simmilar.
        setOpen(false);
    };

    const handleAddMember = async () => {
        if (!newMemberId.trim()) {
            alert("Please enter a valid user ID.");
            return;
        }

        try {
            await patchGroupMember(groupInfo.id, {
                addMemberUserId: Number(newMemberId),
            });
            setNewMemberId("");
            window.location.reload();
        } catch (err) {
            console.error("Failed to add member:", err);
            alert("Failed to add member.");
        }
    };

    const handleRemoveMember = async (memberId: number) => {
        try {
            await patchGroupMember(groupInfo.id, {
                removeMemberUserId: memberId,
            });

            // If you remove yourself, exit group page
            if (memberId === userId) {
                navigate("/groups", { replace: true });
            } else {
                window.location.reload();
            }
        } catch {
            alert("Cannot remove user with outstanding debts.");
        }
    };

    const handleMarkAsPaid = async (targetId: number) => {
        try {
            await patchGroupMember(groupInfo!.id, {
                paidMemberUserId: targetId,
                fromMemberUserId: userId || 0,
            });
            window.location.reload();
        } catch (err) {
            alert("Could not mark as paid.");
        }
    };

    return (
        <div className="payment-container">

            {/* Header */}
            <div className="d-flex justify-content-between pb-3 mx-3">
                <p className="h3">{groupInfo.name}</p>
            </div>

            {/* Members */}
            <div className="d-flex justify-content-between pb-1 mx-3">
                <p className="h4">Members:</p>
                <div className="add-member-container">
                    <input
                        type="number"
                        placeholder="Enter user ID"
                        className="form-control"
                        value={newMemberId}
                        onChange={(e) => setNewMemberId(e.target.value)}
                    />
                    <button
                        className="btn btn-primary"
                        onClick={handleAddMember}
                    >
                        Add
                    </button>
                </div>
            </div>

            {/* Members Table */}
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
                        {groupInfo.members.map((user) => (
                            <tr key={user.id}>
                                <td className="fs-5">{user.name}</td>
                                <td className="fs-5">{user.balance}</td>
                                <td className="fs-5 d-flex gap-2">
                                    <button
                                        className="btn btn-primary"
                                        disabled={user.balance! >= 0}
                                        onClick={() => {handleMarkAsPaid(user.id);}}
                                    >
                                        Pay
                                    </button>
                                    <button
                                        onClick={() => {
                                            setOpen(true);
                                            setFormUser(user.id);
                                        }}
                                        className="btn btn-primary"
                                    >
                                        Add new Payment
                                    </button>
                                    <button
                                        className="btn btn-danger"
                                        onClick={() => {handleRemoveMember(user.id);}}
                                    >
                                        Remove
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {/* Transactions */}
            <div className="d-flex justify-content-between pt-3 pb-1 mx-3">
                <p className="h4">Payments:</p>
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
                        {groupInfo.transactions.map((tx) => {
                            const payer = groupInfo.members.find(u => u.id === tx.payerId);
                            const formattedDate = new Date(tx.date).toISOString()
                                .slice(0, 16)
                                .replace("T", " ");
                            return (
                                <tr key={tx.id}>
                                    <td className="fs-6">{formattedDate}</td>
                                    <td className="fs-6">{payer?.name || "Unknown"}</td>
                                    <td className="fs-6 text-break">{tx.description}</td>
                                    <td className="fs-6">{tx.amount} €</td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>

            {/* Transaction Form */}
            {open && (
                <>
                    <div className="dark-overlay" />
                    <NewTransactionForm
                        onCreate={handleCreateTransaction}
                        onCancel={() => {
                            setOpen(false);
                            setFormUser(0);
                        }}
                        group={groupInfo}
                        from={formUser}
                    />
                </>
            )}
        </div>
    );
}
