import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { leaveTypesApi, leaveRequestsApi } from "@/lib/api";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { SelectNative } from "@/components/ui/select-native";
import { useLocation } from "wouter";

export default function NewLeaveRequestPage() {
  const [, setLocation] = useLocation();
  const queryClient = useQueryClient();
  const [error, setError] = useState("");

  const [leaveTypeId, setLeaveTypeId] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [reason, setReason] = useState("");

  const { data: leaveTypes } = useQuery({
    queryKey: ['leave-types', 'all'],
    queryFn: () => leaveTypesApi.getAll({ pageSize: 100 }).then(res => res.items),
  });

  const createMutation = useMutation({
    mutationFn: leaveRequestsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['my-leave-requests'] });
      queryClient.invalidateQueries({ queryKey: ['my-leave-balances'] });
      setLocation('/my-leave-requests');
    },
    onError: (err: any) => {
      setError(err.statusText || "Failed to submit leave request.");
    }
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    if (!leaveTypeId || !startDate || !endDate || !reason) {
      setError("Please fill in all required fields.");
      return;
    }
    if (new Date(startDate) > new Date(endDate)) {
      setError("Start date must be before or equal to end date.");
      return;
    }
    createMutation.mutate({
      leaveTypeId: Number(leaveTypeId),
      startDate,
      endDate,
      reason
    });
  };

  return (
    <div className="space-y-6 max-w-2xl mx-auto">
      <h1 className="text-3xl font-bold tracking-tight">Apply for Leave</h1>
      
      <Card>
        <CardHeader>
          <CardTitle>Leave Application Form</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && (
              <div className="p-3 text-sm text-destructive-foreground bg-destructive/90 rounded-md">
                {error}
              </div>
            )}
            <div className="space-y-2">
              <Label>Leave Type <span className="text-red-500">*</span></Label>
              <SelectNative value={leaveTypeId} onChange={e => setLeaveTypeId(e.target.value)} required>
                <option value="">Select leave type...</option>
                {leaveTypes?.map(lt => (
                  <option key={lt.id} value={lt.id}>{lt.name}</option>
                ))}
              </SelectNative>
            </div>
            
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>Start Date <span className="text-red-500">*</span></Label>
                <Input type="date" value={startDate} onChange={e => setStartDate(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>End Date <span className="text-red-500">*</span></Label>
                <Input type="date" value={endDate} onChange={e => setEndDate(e.target.value)} required />
              </div>
            </div>

            <div className="space-y-2">
              <Label>Reason <span className="text-red-500">*</span></Label>
              <Textarea 
                value={reason} 
                onChange={e => setReason(e.target.value)} 
                required 
                placeholder="Please briefly explain your reason for leave..."
                className="min-h-[120px]"
              />
            </div>

            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setLocation('/my-leave-requests')}>
                Cancel
              </Button>
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending ? "Submitting..." : "Submit Application"}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
