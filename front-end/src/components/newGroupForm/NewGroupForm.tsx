import { useState } from "react";
import { useAuth } from "../../context/AuthContext";
import type { GroupSimpleDto } from "../../types/Group";
import "./NewGroupForm.css";

interface NewGroupFormProps {
    onCreate: (newGroupPost: GroupSimpleDto) => void;
    onCancel: () => void;
};

export default function NewGroupForm({ onCreate, onCancel }: NewGroupFormProps) {
    const [groupName, setGroupName] = useState("");
    const { userId } = useAuth();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const newGroupPost: GroupSimpleDto = {
            id: null,
            name: groupName,
            creatorId: userId,
            balance: 0
        };

        onCreate(newGroupPost);
        setGroupName("");
    }
    return (
        <div className="form-container">
            <form onSubmit={handleSubmit} className="border border-grey p-3 rounded bg-light">
                <div className="mb-3">
                    <label htmlFor="groupName" className="form-label">Group name</label>
                    <input
                        onChange={(e) => setGroupName(e.target.value)}
                        type="text"
                        className="form-control"
                        id="groupName"
                        required
                    />
                </div>
                <div>
                    <button type="submit" className="btn btn-primary">Create</button>
                    <button type="button" onClick={onCancel} className="btn btn-danger ms-3">Cancel</button>
                </div>

            </form>
        </div>
    );
}