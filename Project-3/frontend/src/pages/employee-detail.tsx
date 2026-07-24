import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { departmentsApi, employeesApi } from "@/lib/api";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { SelectNative } from "@/components/ui/select-native";
import { useState, useEffect } from "react";
import { useParams, useLocation } from "wouter";
import { useAuth } from "@/contexts/AuthContext";
import { formatDate } from "@/lib/utils";

export default function EmployeeDetailPage() {
  const { id } = useParams();
  const [, setLocation] = useLocation();
  const { isAdmin } = useAuth();
  const queryClient = useQueryClient();
  
  const [isEditing, setIsEditing] = useState(false);
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [departmentId, setDepartmentId] = useState<number | "">("");

  const { data: emp, isLoading } = useQuery({
    queryKey: ['employee', id],
    queryFn: () => employeesApi.getById(id!),
    enabled: !!id,
  });

  const { data: depts } = useQuery({
    queryKey: ['departments'],
    queryFn: departmentsApi.getAll,
  });

  useEffect(() => {
    if (emp && !isEditing) {
      setFirstName(emp.firstName);
      setLastName(emp.lastName);
      setDepartmentId(emp.departmentId || "");
    }
  }, [emp, isEditing]);

  const updateMutation = useMutation({
    mutationFn: (data: any) => employeesApi.update(id!, data),
    onSuccess: (data) => {
      queryClient.setQueryData(['employee', id], data);
      queryClient.invalidateQueries({ queryKey: ['employees'] });
      setIsEditing(false);
    }
  });

  const deleteMutation = useMutation({
    mutationFn: () => employeesApi.delete(id!),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
      setLocation('/employees');
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
  if (!emp) return <div>Employee not found.</div>;

  return (
    <div className="space-y-6 max-w-2xl mx-auto">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold tracking-tight">Employee Details</h1>
        {isAdmin && !isEditing && (
          <div className="space-x-2">
            <Button variant="outline" onClick={() => setIsEditing(true)}>Edit</Button>
            <Button variant="destructive" onClick={() => {
              if(confirm('Delete this employee? This action cannot be undone.')) {
                deleteMutation.mutate();
              }
            }} disabled={deleteMutation.isPending}>Delete</Button>
          </div>
        )}
      </div>
      
      <Card>
        <CardHeader>
          <CardTitle>Profile Information</CardTitle>
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
                <Button type="submit" disabled={updateMutation.isPending}>Save Changes</Button>
              </div>
            </form>
          ) : (
            <div className="grid grid-cols-2 gap-y-6">
              <div>
                <p className="text-sm text-muted-foreground">Full Name</p>
                <p className="font-medium text-lg mt-1">{emp.fullName}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Email</p>
                <p className="font-medium text-lg mt-1">{emp.email}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Department</p>
                <p className="font-medium text-lg mt-1">{emp.departmentName || "None"}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">System Roles</p>
                <p className="font-medium text-lg mt-1">{emp.roles.join(', ') || "Employee"}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Hire Date</p>
                <p className="font-medium text-lg mt-1">{formatDate(emp.hireDate)}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Date of Birth</p>
                <p className="font-medium text-lg mt-1">{formatDate(emp.dateOfBirth)}</p>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
