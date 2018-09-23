function  [filteredsignal]=gaussfilter(signal,fs,w,sigma)
time=(1:length(signal))./fs;
signal=signal-mean(signal);
signal=signal./max(signal);
w(1:w)=1;
for i=1:2:length(w)
    w(i)=(w(i)+sigma);
    end

filteredsignal=conv(signal,w);
filteredsignal=filteredsignal-mean(filteredsignal);
filteredsignal=filteredsignal./max(filteredsignal);
end