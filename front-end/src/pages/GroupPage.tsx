import { useState, useEffect} from "react";
import { useParams } from 'react-router-dom';


export default function GroupPage() {
    const { groupId } = useParams();
    const [open, setOpen] = useState(false);
    useEffect(() => {

    }, []);



    return (
        <div className="groupContainer">
            
            <div className="list-group-item d-flex justify-content-between pb-3 mx-3">
                <p className="h3">Groups</p>
                <button
                    onClick={(() => setOpen(true))}
                    className="btn btn-primary">
                    Add new
                </button>
            </div>

            <ul className="list-group">
                {/* {groups.map((group) => (
                    <li key={group.id} className="list-group-item d-flex justify-content-between">
                        <span className="fs-5">{group.name}</span>
                        <button className="btn btn-secondary">View</button>
                    </li>
                ))} */}
            </ul>

            {open &&
                (
                    <>
                        <div className="dark-overlay" />
                        {/* <NewGroupForm onCreate={handleCreate} onCancel={() => setOpen(false)} /> */}
                    </>
                )}

        </div>
    );
}



