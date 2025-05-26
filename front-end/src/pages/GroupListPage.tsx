import { useEffect, useState } from "react";
import { fetchGroups } from "../api/groupsAPI.ts";
import { useNavigate } from "react-router-dom";
import type { GroupSimpleDto } from "../types/Group.ts";
import NewGroupForm from "../components/newGroupForm/NewGroupForm.tsx";
import { postGroup } from "../api/groupsAPI.ts";
import { useAuth } from "../context/AuthContext.tsx";
import "./GroupListPage.css";

export default function GroupListPage() {
  const [groups, setGroups] = useState<GroupSimpleDto[]>([]);
  const [open, setOpen] = useState(false);
  const { isLoggedIn, userId } = useAuth();
  const navigate = useNavigate();


  useEffect(() => {
    if (!isLoggedIn || userId === null) {
      navigate("/login", { replace: true });
    }
    else {
      fetchGroups(userId).then(( data) => { setGroups(data); })
    }
  }, [isLoggedIn, navigate]);

  function handleCreate(newGroupPost: GroupSimpleDto) {
    postGroup(newGroupPost)
      .then((newGroup) => {
        setGroups((prevGroups) => [...prevGroups, newGroup])
      })
      .catch((error) => {
        console.error("Failed to add group:", error);
      });
    setOpen(false);
  }


  return (
    <div className="group-container">
      <div className="d-flex justify-content-between pb-3 mx-3">
        <p className="h3">Groups</p>
        <button onClick={() => setOpen(true)} className="btn btn-primary">
          Add new
        </button>
      </div>

      <div className="table-responsive mx-3">
        <table className="table table-bordered">
          <thead className="table-light">
            <tr>
              <th>Group name</th>
              <th>Balance</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {groups.map((group) => (
              <tr key={group.id} onClick={() => navigate(`/group/${group.id}`)} style={{ cursor: 'pointer' }}>
                <td>{group.name}</td>
                <td>{group.balance}</td>
                <td>
                  <button
                    className="btn btn-secondary"
                    onClick={(e) => {
                      e.stopPropagation(); // prevent row click
                      navigate(`/group/${group.id}`);
                    }}
                  >
                    View
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {open && (
        <>
          <div className="dark-overlay" />
          <NewGroupForm onCreate={handleCreate} onCancel={() => setOpen(false)} />
        </>
      )}
    </div>
  );
}