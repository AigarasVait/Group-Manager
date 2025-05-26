import { useState } from "react";
import { useAuth } from "../../context/AuthContext";
import type { GroupSimpleDto } from "../../types/Group";
import "./NewGroupForm.css";

interface NewGroupFormProps {
    onCreate: (newGroupPost: GroupSimpleDto) => void;
    onCancel: () => void;
}

export default function NewGroupForm({ onCreate, onCancel }: NewGroupFormProps) {
    const [groupName, setGroupName] = useState("");
    const { userId } = useAuth();

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        const trimmedName = groupName.trim();
        if (!trimmedName || userId == null) return;

        const newGroupPost: GroupSimpleDto = {
            id: null,
            name: trimmedName,
            creatorId: userId,
            balance: 0,
        };

        onCreate(newGroupPost);
        setGroupName("");
    };

    return (
        <div className="form-container">
            <form onSubmit={handleSubmit} className="border border-grey p-3 rounded bg-light">
                <div className="mb-3">
                    <label htmlFor="groupName" className="form-label">Group name</label>
                    <input
                        id="groupName"
                        type="text"
                        className="form-control"
                        value={groupName}
                        onChange={(e) => setGroupName(e.target.value)}
                        required
                    />
                </div>
                <div className="d-flex justify-content-end gap-2">
                    <button
                        type="submit"
                        className="btn btn-primary"
                        disabled={groupName.trim() === ""}
                    >
                        Create
                    </button>
                    <button
                        type="button"
                        onClick={onCancel}
                        className="btn btn-danger"
                    >
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    );
}
