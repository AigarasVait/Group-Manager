import { useState } from "react";
import type { GroupPost } from "../../types/Group";
import "./NewGroupForm.css";

interface NewGroupFormProps {
    onCreate: (newGroupPost: GroupPost) => void;
    onCancel: () => void;
};

export default function NewGroupForm({ onCreate, onCancel }: NewGroupFormProps) {
    const [groupName, setGroupName] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const newGroupPost: GroupPost = {
            name: groupName,
            creatorId: 1
        };

        onCreate(newGroupPost);
        setGroupName("");
    }
    return (
        <div className="form-container">
            <form onSubmit={handleSubmit} className="border border-grey p-3 rounded bg-light">
                <div className="mb-3">
                    <label htmlFor="groupName" className="form-label">Group name</label>
                    <input onChange={(e) => setGroupName(e.target.value)} type="text" className="form-control" id="groupName" />
                </div>
                <div>
                    <button type="submit" className="btn btn-primary">Create</button>
                    <button type="button" onClick={onCancel} className="btn btn-danger ms-3">Cancel</button>
                </div>

            </form>
        </div>
    );
}