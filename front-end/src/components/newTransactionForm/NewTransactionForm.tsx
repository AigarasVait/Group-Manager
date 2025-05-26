import { useState } from "react";
import { useAuth } from "../../context/AuthContext";
import type { TransactionCreateDto } from "../../types/Transaction";
import type { GroupDto } from "../../types/Group";
import "./NewTransactionForm.css";

interface NewGroupFormProps {
    onCreate: (newTransaction: TransactionCreateDto) => void;
    onCancel: () => void;
    group?: GroupDto;
};

export default function NewGroupForm({ onCreate, onCancel, group }: NewGroupFormProps) {
    const [amountIn, setAmountIn] = useState(0);
    const membersCount = group?.members.length || 0;
    const [values, setValues] = useState<number[]>(Array(membersCount).fill(0));
    const [descriptionItem, setDescriptionItem] = useState("");
    //                                     0 - equal, 1 - dynamic, 2 - percentage
    const [splitTypeItem, setSplitTypeItem] = useState<0 | 1 | 2 | null>(null);
    const { userId } = useAuth();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (splitTypeItem === null || splitTypeItem === undefined) {
            alert("Please select a split type.");
            return;
        }

        const newTransaction: TransactionCreateDto = {
            groupId: group?.id || null,
            payerId: userId,
            amount: amountIn,
            description: descriptionItem,
            sType: splitTypeItem,
            splitValues: values
        };

        onCreate(newTransaction);
        setDescriptionItem("");
        setSplitTypeItem(null);
        setAmountIn(0);
    }
    return (
        <div className="form-container">
            <form onSubmit={handleSubmit} className="border border-grey p-3 rounded bg-light py-4 px-4">
                <p className="h4">New payment</p>
                <div className="mb-3">
                    <label htmlFor="description" className="form-label">Description</label>
                    <input
                        onChange={(e) => setDescriptionItem(e.target.value)}
                        type="text"
                        className="form-control"
                        id="description" />
                </div>
                <div className="mb-3">
                    <label htmlFor="amount" className="form-label">Amount</label>
                    <input
                        onChange={(e) => setAmountIn(Number(e.target.value))}
                        type="number"
                        step="0.01"
                        className="form-control"
                        required
                        id="amount" />

                </div>



                <div>
                    <div className="mb-3 form-check">
                        <input
                            type="checkbox"
                            className="form-check-input"
                            id="equalSplit"
                            checked={splitTypeItem === 0} // 0 for equal split
                            onChange={() => setSplitTypeItem(0)} // 0 for equal split
                        />
                        <label className="form-check-label" htmlFor="equalSplit">Equal split</label>
                    </div>

                    <div className="mb-3 form-check">
                        <input
                            type="checkbox"
                            className="form-check-input"
                            id="dynamicSplit"
                            checked={splitTypeItem === 1} // 1 for dynamic split
                            onChange={() => setSplitTypeItem(1)} // 1 for dynamic split
                        />
                        <label className="form-check-label" htmlFor="dynamicSplit">Dynamic</label>
                    </div>

                    <div className="mb-3 form-check">
                        <input
                            type="checkbox"
                            className="form-check-input"
                            id="percentageSplit"
                            checked={splitTypeItem === 2} // 2 for percentage split
                            onChange={() => setSplitTypeItem(2)} // 2 for percentage split
                        />
                        <label className="form-check-label" htmlFor="percentageSplit">Percentage</label>
                    </div>

                    {/*  percentage split      dynamic split       */}
                    {(splitTypeItem === 2 || splitTypeItem === 1) && (
                        <div className="mb-3">
                            {values.map((val, index) => (
                                <div key={index}>
                                    <label className="form-label">{group?.members[index]?.name}</label>
                                    <input
                                        type="number"
                                        step="0.01"
                                        required
                                        value={val}
                                        onChange={(e) => {
                                            const newValues = [...values];
                                            newValues[index] = Number(e.target.value);
                                            setValues(newValues);
                                        }}
                                        className="form-control mb-2"
                                    />
                                </div>
                            ))}
                        </div>
                    )}
                </div>



                <div>
                    <button type="submit" className="btn btn-primary">Create</button>
                    <button type="button" onClick={onCancel} className="btn btn-danger ms-3">Cancel</button>
                </div>

            </form>
        </div>
    );
}