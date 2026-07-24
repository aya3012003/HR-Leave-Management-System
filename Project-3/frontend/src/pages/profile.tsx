import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { employeesApi, departmentsApi } from "@/lib/api";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { SelectNative } from "@/components/ui/select-native";
import { useState, useEffect } from "react";
import { formatDate } from "@/lib/utils";

export default function ProfilePage() {
  const queryClient = useQueryClient();
  const [isEditing, setIsEditing] = useState(false);
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [departmentId, setDepartmentId] = useState<number | "">("");

  const { data: me, isLoading } = useQuery({
    queryKey: ['me'],
    queryFn: employeesApi.getMe,
  });

  const { data: depts } = useQuery({
    queryKey: ['departments'],
    queryFn: departmentsApi.getAll,
  });

  useEffect(() => {
    if (me && !isEditing) {
      setFirstName(me.firstName);
      setLastName(me.lastName);
      setDepartmentId(me.departmentId || "");
    }
  }, [me, isEditing]);

  const updateMutation = useMutation({
    mutationFn: employeesApi.updateMe,
    onSuccess: (data) => {
      queryClient.setQueryData(['me'], data);
      setIsEditing(false);
    }
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateMutation.mutate({
      firstName,
      lastName,
      departmentId: departmentId ? Number(departmentId) : undefined,
    });
  };

  if (isLoading) return <div>Loading...</div>;
  if (!me) return <div>Failed to load profile.</div>;

  return (
    <div className="space-y-6 max-w-2xl mx-auto">
      <h1 className="text-3xl font-bold tracking-tight">My Profile</h1>
      
      <Card>
        <CardHeader className="flex flex-row justify-between items-center">
          <CardTitle>Personal Information</CardTitle>
          {!isEditing && (
            <Button variant="outline" size="sm" onClick={() => setIsEditing(true)}>
              Edit Profile
            </Button>
          )}
        </CardHeader>
        <CardContent>
          {isEditing ? (
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>First Name</Label>
                  <Input value={firstName} onChange={e => setFirstName(e.target.value)} required />
                </div>
                <div className="space-y-2">
                  <Label>Last Name</Label>
                  <Input value={lastName} onChange={e => setLastName(e.target.value)} required />
                </div>
              </div>
              <div className="space-y-2">
                <Label>Department</Label>
                <SelectNative value={departmentId} onChange={e => setDepartmentId(e.target.value)}>
                  <option value="">Select a department...</option>
                  {depts?.map(d => (
                    <option key={d.id} value={d.id}>{d.name}</option>
                  ))}
                </SelectNative>
              </div>
              <div className="flex justify-end gap-2 pt-4">
                <Button type="button" variant="ghost" onClick={() => setIsEditing(false)}>Cancel</Button>
                <Button type="submit" disabled={updateMutation.isPending}>
                  {updateMutation.isPending ? "Saving..." : "Save Changes"}
                </Button>
              </div>
            </form>
          ) : (
            <div className="space-y-6">
              <div className="grid grid-cols-2 gap-y-4">
                <div>
                  <p className="text-sm text-muted-foreground">Full Name</p>
                  <p className="font-medium">{me.fullName}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Email</p>
                  <p className="font-medium">{me.email}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Department</p>
                  <p className="font-medium">{me.departmentName || "None"}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Roles</p>
                  <p className="font-medium">{me.roles.join(', ') || "Employee"}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Hire Date</p>
                  <p className="font-medium">{formatDate(me.hireDate)}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Date of Birth</p>
                  <p className="font-medium">{formatDate(me.dateOfBirth)}</p>
                </div>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
