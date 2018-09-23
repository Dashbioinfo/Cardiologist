function [coeff]=correlation(signal1,signal2);
if length(signal1)==length(signal2)
coeff=(mean(signal1.*signal2')-(mean(signal1)*mean(signal2)))./(std(signal1)*std(signal2));
else
    coeff=0;
end
