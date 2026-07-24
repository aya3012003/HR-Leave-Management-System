import { useQuery } from "@tanstack/react-query";
import { leaveBalancesApi } from "@/lib/api";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { WalletCards } from "lucide-react";

export default function MyLeaveBalancesPage() {
  const { data: balances, isLoading } = useQuery({
    queryKey: ['my-leave-balances'],
    queryFn: leaveBalancesApi.getMy,
  });

  if (isLoading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">My Leave Balances</h1>
        <p className="text-muted-foreground mt-1">Check your remaining leave quotas for the year.</p>
      </div>

      {balances && balances.length > 0 ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {balances.map(balance => (
            <Card key={balance.id} className="overflow-hidden">
              <CardHeader className="bg-muted/50 pb-4 border-b">
                <CardTitle className="text-lg flex items-center gap-2">
                  <WalletCards size={18} className="text-primary" />
                  {balance.leaveTypeName}
                </CardTitle>
              </CardHeader>
              <CardContent className="p-6 flex flex-col items-center justify-center text-center">
                <p className="text-5xl font-bold text-foreground">{balance.remainingDays}</p>
                <p className="text-sm text-muted-foreground mt-2 font-medium tracking-wide uppercase">Days Remaining</p>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : (
        <div className="text-center py-12 text-muted-foreground bg-muted/20 rounded-lg border border-dashed">
          No leave balances found. Contact HR if you believe this is an error.
        </div>
      )}
    </div>
  );
}
