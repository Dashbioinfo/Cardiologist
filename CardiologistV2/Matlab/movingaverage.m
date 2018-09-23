function [filteredsignal]=movingaverage(signal,fs,w);
% signal=signal-mean(signal);
% signal=signal./max(signal);
    g=round(w/2);
    Time=(1:length(signal))./fs;
    INT(1:length(signal))=0;
for i=(g+1):((length(signal))-g)
    INT(i)=sum(signal(i-g:i+g));
    INT(i)=INT(i)./w;
end
INT(1:g)=INT(g+1);
INT((length(signal)-g):length(signal))=INT(length(signal)-g+1);
filteredsignal=INT;
filteredsignal1=filteredsignal-mean(filteredsignal);
filteredsignal1=filteredsignal1./max(filteredsignal1);
figure
subplot(2,1,1)
hold on
title('Normalized Signal(Time Domain)')
plot(Time,signal)
subplot(2,1,2)
hold on
title('Filtered Signal(Time Domain)')
plot(Time,filteredsignal1);