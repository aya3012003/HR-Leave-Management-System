import { useState, useEffect } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { employeesApi } from "@/lib/api";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useToast } from "@/hooks/use-toast";
import { formatDate } from "@/lib/utils";

export default function ProfilePage() {
    const queryClient = useQueryClient();
    const { toast } = useToast();

    const [isEditing, setIsEditing] = useState(false);
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");

    const { data: profile, isLoading } = useQuery({
        queryKey: ['my-profile'],
        queryFn: employeesApi.getMe,
    });

    useEffect(() => {
        if (profile && !isEditing) {
            setFirstName(profile.firstName || "");
            setLastName(profile.lastName || "");
        }
    }, [profile, isEditing]);

    const updateMutation = useMutation({
        mutationFn: (data: { firstName: string, lastName: string }) => employeesApi.updateMe(data),
        onSuccess: (data) => {
            queryClient.setQueryData(['my-profile'], data);
            setIsEditing(false);
            toast({ title: "Profile updated successfully." });
        },
        onError: () => {
            toast({ title: "Failed to update profile.", variant: "destructive" });
        }
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        updateMutation.mutate({ firstName, lastName });
    };

    if (isLoading) return <div>Loading profile...</div>;
    if (!profile) return <div>Failed to load profile data.</div>;

    return (
        <div className="space-y-6 max-w-2xl mx-auto">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">My Profile</h1>
                {!isEditing && (
                    <Button variant="outline" onClick={() => setIsEditing(true)}>Edit Profile</Button>
                )}
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Personal Information</CardTitle>
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

                            <div className="flex justify-end gap-2 pt-4">
                                <Button type="button" variant="ghost" onClick={() => setIsEditing(false)}>Cancel</Button>
                                <Button type="submit" disabled={updateMutation.isPending}>
                                    {updateMutation.isPending ? "Saving..." : "Save Changes"}
                                </Button>
                            </div>
                        </form>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-y-6">
                            <div>
                                <p className="text-sm text-muted-foreground">Full Name</p>
                                <p className="font-medium text-lg mt-1">{profile.fullName}</p>
                            </div>
                            <div>
                                <p className="text-sm text-muted-foreground">Email</p>
                                <p className="font-medium text-lg mt-1">{profile.email}</p>
                            </div>
                            <div>
                                <p className="text-sm text-muted-foreground">Department</p>
                                <p className="font-medium text-lg mt-1">{profile.departmentName || "Unassigned"}</p>
                            </div>
                            <div>
                                <p className="text-sm text-muted-foreground">System Roles</p>
                                <p className="font-medium text-lg mt-1">{profile.roles?.join(', ') || "Employee"}</p>
                            </div>
                            <div>
                                <p className="text-sm text-muted-foreground">Hire Date</p>
                                <p className="font-medium text-lg mt-1">{formatDate(profile.hireDate)}</p>
                            </div>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
}