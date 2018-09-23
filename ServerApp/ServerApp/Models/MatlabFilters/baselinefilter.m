function [full]=baselinefilter(signal,fs)
index = signal(:,1);
lead2 = signal(:,2);
v1 = signal(:,3);

lead2=lead2-mean(lead2);
lead2=lead2./max(lead2);
time=(1:length(lead2))./fs;

[f]=gaussfilter(lead2,fs,500,0.1);
close
f1=f(250:(length(lead2))+249);
[Factor]=fitting(lead2,f1);
%B=((signal'*signal).^-1)*signal'*f1;
f2=f1./Factor;
[coeff]=correlation(lead2,f2');
if coeff>0.85
 filteredsignal=lead2-f2;
else
[f2]=movingaverage(lead2,370,200);
close
end
z1=size(f2);
z2=size(lead2);
if z1==z2
filteredsignal=lead2-f2;
else
filteredsignal=lead2-f2';
end
full=[index filteredsignal v1];    
end
